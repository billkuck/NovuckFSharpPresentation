import { defineConfig } from 'vite';
import tailwindcss from '@tailwindcss/vite';
const proxyPort = process.env.SERVER_PROXY_PORT || "5507";
const proxyTarget = "http://localhost:" + proxyPort;

export default defineConfig({
    plugins: [
        tailwindcss(),
    ],
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