function readAbsoluteUrl(
  variableName: string,
  configuredValue: string | undefined,
): string {
  const normalizedValue = configuredValue?.trim();

  if (!normalizedValue) {
    throw new Error(`${variableName} is required.`);
  }

  let parsedUrl: URL;

  try {
    parsedUrl = new URL(normalizedValue);
  } catch {
    throw new Error(`${variableName} must be a valid absolute URL.`);
  }

  return parsedUrl.toString().replace(/\/$/, "");
}

export const environment = Object.freeze({
  apiBaseUrl: readAbsoluteUrl(
    "VITE_API_BASE_URL",
    import.meta.env.VITE_API_BASE_URL,
  ),
  signalRHubUrl: readAbsoluteUrl(
    "VITE_SIGNALR_HUB_URL",
    import.meta.env.VITE_SIGNALR_HUB_URL,
  ),
});
