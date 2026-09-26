function readApplicationUrl(
  variableName: string,
  configuredValue: string | undefined,
): string {
  const normalizedValue = configuredValue?.trim();

  if (!normalizedValue) {
    throw new Error(`${variableName} is required.`);
  }

  const isAbsoluteUrl = /^https?:\/\//i.test(normalizedValue);
  const isRootRelativeUrl = normalizedValue.startsWith("/");

  if (!isAbsoluteUrl && !isRootRelativeUrl) {
    throw new Error(
      `${variableName} must be an absolute HTTP URL or a root-relative path.`,
    );
  }

  let parsedUrl: URL;

  try {
    parsedUrl = new URL(normalizedValue, window.location.origin);
  } catch {
    throw new Error(`${variableName} must be a valid application URL.`);
  }

  if (parsedUrl.protocol !== "http:" && parsedUrl.protocol !== "https:") {
    throw new Error(`${variableName} must use HTTP or HTTPS.`);
  }

  return parsedUrl.toString().replace(/\/$/, "");
}

export const environment = Object.freeze({
  apiBaseUrl: readApplicationUrl(
    "VITE_API_BASE_URL",
    import.meta.env.VITE_API_BASE_URL,
  ),
  signalRHubUrl: readApplicationUrl(
    "VITE_SIGNALR_HUB_URL",
    import.meta.env.VITE_SIGNALR_HUB_URL,
  ),
});
