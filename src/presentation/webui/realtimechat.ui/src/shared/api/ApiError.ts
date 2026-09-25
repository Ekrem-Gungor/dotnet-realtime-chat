import type { ApiProblemDetails } from "./apiProblemDetails";

export class ApiError extends Error {
  readonly status: number;
  readonly problemDetails?: ApiProblemDetails;

  constructor(status: number, problemDetails?: ApiProblemDetails) {
    super(
      problemDetails?.detail ??
        problemDetails?.title ??
        `API request failed with status ${status}.`,
    );

    this.name = "ApiError";
    this.status = status;
    this.problemDetails = problemDetails;
  }
}
