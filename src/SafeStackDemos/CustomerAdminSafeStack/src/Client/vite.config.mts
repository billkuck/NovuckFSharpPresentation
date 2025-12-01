import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

const proxyPort = process.env.SERVER_PROXY_PORT || "5507";
const proxyTarget = "http://localhost:" + proxyPort;

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [react()],
    build: {
        outDir: "../../deploy/public",
    },
    server: {
        port: 8807,
        proxy: {
            // redirect requests that start with /api/ to the server on port 5507
            "/api/": {
                target: proxyTarget,
                changeOrigin: true,
            }
        },
        watch: {
            ignored: [ "**/*.fs" ]
        },
    }
});