import { HttpErrorResponse } from '@angular/common/http';

export function getErrorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse && error.error?.detail) {
    return error.error.detail;
  }

  return 'Something went wrong. Please try again.';
}
