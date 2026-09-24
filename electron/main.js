const path = require("node:path");
const { spawn } = require("node:child_process");
const { app, BrowserWindow, shell } = require("electron");

const API_URL = "http://localhost:5050";
const FRONTEND_DEV_URL = process.env.ELECTRON_RENDERER_URL || "http://localhost:5173";
const isDevelopment = process.env.NODE_ENV === "development";

let backendProcess = null;
const backendLogs = [];
const BACKEND_LOG_LIMIT = 120;

function appendBackendLog(source, data) {
    const lines = String(data)
        .split(/\r?\n/)
        .map((line) => line.trim())
        .filter(Boolean);

    for (const line of lines) {
        backendLogs.push(`[${source}] ${line}`);
    }

    if (backendLogs.length > BACKEND_LOG_LIMIT) {
        backendLogs.splice(0, backendLogs.length - BACKEND_LOG_LIMIT);
    }
}

function getRecentBackendLogs() {
    if (backendLogs.length === 0) {
        return "No backend logs captured.";
    }

    return backendLogs.slice(-30).join("\n");
}

function delay(ms) {
    return new Promise((resolve) => setTimeout(resolve, ms));
}

async function waitForBackendReady(timeoutMs = 30000, intervalMs = 500) {
    const deadline = Date.now() + timeoutMs;
    let lastError = "";

    while (Date.now() < deadline) {
        if (backendProcess && backendProcess.exitCode !== null) {
            throw new Error(`Backend exited early with code ${backendProcess.exitCode}.`);
        }

        try {
            const response = await fetch(`${API_URL}/health`, {
                cache: "no-store",
            });

            if (response.ok) {
                return;
            }

            lastError = `Health check returned HTTP ${response.status}.`;
        } catch (error) {
            lastError = String(error);
        }

        await delay(intervalMs);
    }

    throw new Error(
        `Timed out waiting for backend at ${API_URL}. Last check: ${lastError || "unknown"}`,
    );
}

function showRendererErrorPage(window, details) {
    const errorHtml = `
<!doctype html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>ChecklistDesktop - Load Error</title>
    <style>
        :root {
            --bg: #f5f7fb;
            --panel: #ffffff;
            --text: #1b2430;
            --muted: #4b5b70;
            --accent: #0f6cbd;
            --danger: #b42318;
            --border: #d9e1ec;
        }
        * { box-sizing: border-box; }
        body {
            margin: 0;
            min-height: 100vh;
            display: grid;
            place-items: center;
            padding: 24px;
            background: radial-gradient(circle at 20% 20%, #e8f1ff, transparent 50%), var(--bg);
            color: var(--text);
            font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
        }
        .card {
            width: min(760px, 100%);
            background: var(--panel);
            border: 1px solid var(--border);
            border-radius: 14px;
            padding: 22px;
            box-shadow: 0 14px 30px rgba(10, 40, 80, 0.08);
        }
        h1 {
            margin: 0 0 10px;
            font-size: 1.4rem;
        }
        p {
            margin: 0 0 12px;
            color: var(--muted);
            line-height: 1.45;
        }
        .error {
            color: var(--danger);
            font-weight: 600;
        }
        .details {
            margin-top: 8px;
            padding: 12px;
            background: #f8fafc;
            border: 1px solid var(--border);
            border-radius: 10px;
            font-family: ui-monospace, SFMono-Regular, Menlo, Consolas, monospace;
            font-size: 12px;
            white-space: pre-wrap;
            word-break: break-word;
        }
        button {
            margin-top: 14px;
            border: none;
            border-radius: 10px;
            background: var(--accent);
            color: #fff;
            padding: 10px 14px;
            font-weight: 600;
            cursor: pointer;
        }
        button:hover {
            filter: brightness(0.96);
        }
    </style>
</head>
<body>
    <main class="card">
        <h1>The app UI could not load</h1>
        <p class="error">Renderer startup failed.</p>
        <p>This usually means frontend files were not found or failed to execute.</p>
        <div class="details">${String(details || "No details available.")}</div>
        <button id="retry">Retry</button>
    </main>
    <script>
        document.getElementById("retry")?.addEventListener("click", () => {
            location.reload();
        });
    </script>
</body>
</html>`;

    window.loadURL(`data:text/html;charset=UTF-8,${encodeURIComponent(errorHtml)}`);
}

// Starts the backend API as a child process when the desktop app launches.
function startBackendProcess() {
    if (isDevelopment) {
        const backendProjectPath = path.join(__dirname, "..", "backend", "Checklist.Api.csproj");

        const args = [
            "run",
            "--project",
            backendProjectPath,
            "--urls",
            API_URL,
        ];

        backendProcess = spawn("dotnet", args, {
            stdio: "inherit",
            shell: process.platform === "win32",
        });
    } else {
        // Packaged builds run a published backend executable from extraResources.
        const backendExecutableName = process.platform === "win32" ? "Checklist.Api.exe" : "Checklist.Api";
        const backendExecutablePath = path.join(process.resourcesPath, "backend", backendExecutableName);

        backendProcess = spawn(backendExecutablePath, ["--urls", API_URL], {
            stdio: ["ignore", "pipe", "pipe"],
            shell: false,
        });

        backendProcess.stdout?.on("data", (data) => {
            appendBackendLog("stdout", data);
        });

        backendProcess.stderr?.on("data", (data) => {
            appendBackendLog("stderr", data);
        });
    }

    backendProcess.on("error", (error) => {
        // Logging early backend start failures makes local debugging much easier.
        console.error("Failed to start backend process:", error);
    });

    backendProcess.on("exit", (code) => {
        console.log(`Backend process exited with code ${code}`);
    });
}

// Stops the backend process so the app does not leave orphan background tasks.
function stopBackendProcess() {
    if (!backendProcess) {
        return;
    }

    backendProcess.kill();
    backendProcess = null;
}

// Creates the main desktop window and points it to dev server or built frontend assets.
function createMainWindow() {
    const window = new BrowserWindow({
        width: 1100,
        height: 760,
        title: "ChecklistDesktop",
        webPreferences: {
            preload: path.join(__dirname, "preload.js"),
            contextIsolation: true,
            nodeIntegration: false,
        },
    });

    let showingFallback = false;

    if (isDevelopment) {
        window.loadURL(FRONTEND_DEV_URL);
    } else {
        const rendererPath = path.join(process.resourcesPath, "frontend-dist", "index.html");
        window.loadFile(rendererPath);
    }

    window.webContents.on("did-fail-load", (_event, errorCode, errorDescription, validatedURL) => {
        console.error("Renderer failed to load:", {
            errorCode,
            errorDescription,
            validatedURL,
        });
        if (!showingFallback) {
            showingFallback = true;
            showRendererErrorPage(
                window,
                `URL: ${String(validatedURL || "unknown")}\nError: ${String(errorDescription || "unknown")}\nCode: ${String(errorCode)}`,
            );
        }
    });

    // Catch blank-window cases where index.html loads but renderer scripts fail at runtime.
    window.webContents.on("did-finish-load", () => {
        setTimeout(async () => {
            if (showingFallback || window.isDestroyed()) {
                return;
            }

            try {
                const health = await window.webContents.executeJavaScript(`(() => {
                    const root = document.getElementById("root");
                    return {
                        location: window.location.href,
                        title: document.title,
                        hasRoot: Boolean(root),
                        rootChildren: root ? root.childElementCount : -1,
                        rootTextLength: root ? root.textContent.trim().length : -1,
                    };
                })();`);

                if (health.hasRoot && health.rootChildren === 0 && health.rootTextLength === 0) {
                    showingFallback = true;
                    showRendererErrorPage(
                        window,
                        `Window loaded but app root is empty.\nURL: ${health.location}\nTitle: ${health.title}\nPossible cause: missing or broken frontend JS bundle.`,
                    );
                }
            } catch (error) {
                showingFallback = true;
                showRendererErrorPage(window, `Renderer health-check error: ${String(error)}`);
            }
        }, 1500);
    });

    window.webContents.setWindowOpenHandler(({ url }) => {
        shell.openExternal(url);
        return { action: "deny" };
    });
}

app.whenReady().then(() => {
    startBackendProcess();

    waitForBackendReady()
        .then(() => {
            createMainWindow();
        })
        .catch((error) => {
            const window = new BrowserWindow({
                width: 960,
                height: 700,
                title: "ChecklistDesktop - Startup Error",
                webPreferences: {
                    preload: path.join(__dirname, "preload.js"),
                    contextIsolation: true,
                    nodeIntegration: false,
                },
            });

            const details = [
                `Backend startup check failed: ${String(error)}`,
                "",
                "Recent backend logs:",
                getRecentBackendLogs(),
            ].join("\n");

            showRendererErrorPage(window, details);
        });

    app.on("activate", () => {
        if (BrowserWindow.getAllWindows().length === 0) {
            createMainWindow();
        }
    });
});

app.on("window-all-closed", () => {
    if (process.platform !== "darwin") {
        app.quit();
    }
});

app.on("before-quit", () => {
    stopBackendProcess();
});
