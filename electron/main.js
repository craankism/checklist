const path = require("node:path");
const { spawn } = require("node:child_process");
const { app, BrowserWindow, shell } = require("electron");

const API_URL = "http://localhost:5050";
const FRONTEND_DEV_URL = process.env.ELECTRON_RENDERER_URL || "http://localhost:5173";
const isDevelopment = process.env.NODE_ENV === "development";

let backendProcess = null;

// Starts the backend API as a child process when the desktop app launches.
function startBackendProcess() {
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
            stdio: "inherit",
            shell: false,
        });
    }
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
        webPreferences: {
            preload: path.join(__dirname, "preload.js"),
            contextIsolation: true,
            nodeIntegration: false,
        },
    });

    if (isDevelopment) {
        window.loadURL(FRONTEND_DEV_URL);
    } else {
        const rendererPath = path.join(process.resourcesPath, "frontend-dist", "index.html");
        window.loadFile(rendererPath);
    }

    window.webContents.setWindowOpenHandler(({ url }) => {
        shell.openExternal(url);
        return { action: "deny" };
    });
}

app.whenReady().then(() => {
    startBackendProcess();
    createMainWindow();

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
