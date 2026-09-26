import { afterEach, describe, expect, it, vi } from "vitest";

describe("environment", () => {
  afterEach(() => {
    vi.unstubAllEnvs();
    vi.resetModules();
  });

  it("resolves root-relative production routes against the current origin", async () => {
    vi.stubEnv("VITE_API_BASE_URL", "/");
    vi.stubEnv("VITE_SIGNALR_HUB_URL", "/chatHub");

    const { environment } = await import("../environment");

    expect(environment.apiBaseUrl).toBe(window.location.origin);
    expect(environment.signalRHubUrl).toBe(
      `${window.location.origin}/chatHub`,
    );
  });

  it("keeps absolute development URLs", async () => {
    vi.stubEnv("VITE_API_BASE_URL", "http://localhost:5258");
    vi.stubEnv(
      "VITE_SIGNALR_HUB_URL",
      "http://localhost:5258/chatHub",
    );

    const { environment } = await import("../environment");

    expect(environment.apiBaseUrl).toBe("http://localhost:5258");
    expect(environment.signalRHubUrl).toBe(
      "http://localhost:5258/chatHub",
    );
  });

  it("rejects non-root-relative paths", async () => {
    vi.stubEnv("VITE_API_BASE_URL", "api");
    vi.stubEnv("VITE_SIGNALR_HUB_URL", "/chatHub");

    await expect(import("../environment")).rejects.toThrow(
      "VITE_API_BASE_URL must be an absolute HTTP URL or a root-relative path.",
    );
  });
});
