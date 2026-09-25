const configuredApiBaseUrl = import.meta.env.VITE_API_BASE_URL?.trim();

if (!configuredApiBaseUrl) {
  throw new Error("VITE_API_BASE_URL is required.");
}

let parsedApiBaseUrl: URL;

try {
  parsedApiBaseUrl = new URL(configuredApiBaseUrl);
} catch {
  throw new Error("VITE_API_BASE_URL must be a valid absolute URL.");
}

export const environment = Object.freeze({
  apiBaseUrl: parsedApiBaseUrl.toString().replace(/\/$/, ""),
});
