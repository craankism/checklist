import { useCallback, useEffect, useState } from "react";
import { TodosPage } from "./pages/TodosPage";
import {
  getGoogleAuthorizeUrl,
  getGoogleConnectionStatus,
} from "./services/googleAuthService";
import { GoogleSignIn } from "./components/GoogleSignIn";

type AuthStatus = "checking" | "connected" | "disconnected";

// Root app component that hosts the todos page.
function App() {
  const [authStatus, setAuthStatus] = useState<AuthStatus>("checking");
  const [isConnecting, setIsConnecting] = useState<boolean>(false);
  const [message, setMessage] = useState<string>("");

  const checkGoogleConnection = useCallback(async () => {
    try {
      const connected = await getGoogleConnectionStatus();
      setAuthStatus(connected ? "connected" : "disconnected");
      return connected;
    } catch (err) {
      const text =
        err instanceof Error
          ? err.message
          : "Could not verify Google login status.";
      setMessage(text);
      setAuthStatus("disconnected");
      return false;
    }
  }, []);

  useEffect(() => {
    checkGoogleConnection();
  }, [checkGoogleConnection]);

  async function waitForGoogleConnection(): Promise<boolean> {
    const timeoutMs = 120000;
    const intervalMs = 2000;
    const startedAt = Date.now();

    while (Date.now() - startedAt < timeoutMs) {
      await new Promise((resolve) => window.setTimeout(resolve, intervalMs));
      const connected = await checkGoogleConnection();
      if (connected) {
        return true;
      }
    }

    return false;
  }

  async function handleSignInWithGoogle() {
    setIsConnecting(true);
    setMessage("");

    try {
      const authorizeUrl = await getGoogleAuthorizeUrl();
      window.open(authorizeUrl, "_blank", "noopener,noreferrer");
      setMessage(
        "Finish Google sign-in in your browser. Waiting for confirmation...",
      );

      const connected = await waitForGoogleConnection();
      if (!connected) {
        setMessage(
          "Google sign-in was not detected yet. Complete sign-in and click Try Again.",
        );
      }
    } catch (err) {
      const text =
        err instanceof Error ? err.message : "Could not start Google sign-in.";
      setMessage(text);
    } finally {
      setIsConnecting(false);
    }
  }

  if (authStatus === "connected") {
    return <TodosPage />;
  }

  return (
    <main className="auth-gate" aria-label="Google login required">
      <section className="auth-gate-card">
        {authStatus === "checking" ? <p>Checking saved login...</p> : null}
        <div className="auth-gate-actions">
          <GoogleSignIn
            handle={handleSignInWithGoogle}
            disabled={isConnecting || authStatus === "checking"}
          />
        </div>
        <div>
          Dev:
          <br></br>
          <button
            type="button"
            onClick={checkGoogleConnection}
            disabled={isConnecting}
          >
            Try Again
          </button>
          {message ? <p className="info-text">{message}</p> : null}
        </div>
      </section>
    </main>
  );
}

export default App;
