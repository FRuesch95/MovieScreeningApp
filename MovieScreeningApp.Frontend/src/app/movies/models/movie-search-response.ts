import { Movie } from './movie';

export interface MovieSearchResponse {
  shows: Movie[];
  hasMore: boolean;
  nextCursor: string | null;
}
