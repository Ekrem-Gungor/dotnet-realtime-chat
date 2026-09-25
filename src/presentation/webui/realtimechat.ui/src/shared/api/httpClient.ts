import { environment } from "../config/environment";
import { ApiError } from "./ApiError";
import type { ApiProblemDetails } from "./apiProblemDetails";

interface ApiRequestOptions extends Omit<RequestInit, "body" | "credentials"> {
  json?: unknown;
}

function createRequestUrl(path: string): string {
  const normalizedPath = path.replace(/^\/+/, "");

  return new URL(normalizedPath, `${environment.apiBaseUrl}/`).toString();
}

async function readProblemDetails(
  response: Response,
): Promise<ApiProblemDetails | undefined> {
  const contentType = response.headers.get("content-type");

  if (!contentType?.includes("json")) {
    return undefined;
  }

  try {
    return (await response.json()) as ApiProblemDetails;
  } catch {
    return undefined;
  }
}

async function readSuccessResponse<T>(response: Response): Promise<T> {
  if (response.status === 204) {
    return undefined as T;
  }

  const contentLength = response.headers.get("content-length");

  if (contentLength === "0") {
    return undefined as T;
  }

  return (await response.json()) as T;
}

export async function apiRequest<T>(
  path: string,
  options: ApiRequestOptions = {},
): Promise<T> {
  const { json, headers: configuredHeaders, ...requestOptions } = options;

  const headers = new Headers(configuredHeaders);

  headers.set("Accept", "application/json, application/problem+json");

  let body: string | undefined;

  if (json !== undefined) {
    headers.set("Content-Type", "application/json");
    body = JSON.stringify(json);
  }

  const response = await fetch(createRequestUrl(path), {
    ...requestOptions,
    credentials: "include",
    headers,
    body,
  });

  if (!response.ok) {
    const problemDetails = await readProblemDetails(response);

    throw new ApiError(response.status, problemDetails);
  }

  return readSuccessResponse<T>(response);
}
