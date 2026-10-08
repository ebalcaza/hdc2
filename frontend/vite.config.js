import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// El puerto 5173 debe coincidir con "Cors:Origenes" en backend.API/appsettings.json
export default defineConfig({
  plugins: [react()],
  server: { port: 5173 }
});
