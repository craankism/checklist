const { contextBridge } = require("electron");

// Exposes a minimal bridge to keep renderer isolation enabled by default.
contextBridge.exposeInMainWorld("desktop", {
    platform: process.platform,
});
