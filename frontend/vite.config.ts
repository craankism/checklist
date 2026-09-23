import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

// https://vite.dev/config/
export default defineConfig({
  // Electron packaged builds load index.html from file://, so assets must be relative.
  base: "./",
  plugins: [react()],
});
