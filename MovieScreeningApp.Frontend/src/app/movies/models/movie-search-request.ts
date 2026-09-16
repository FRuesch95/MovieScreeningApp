export type MovieOrderBy = 'original_title' | 'release_date' | 'rating' | 'popularity_alltime';
export type OrderDirection = 'asc' | 'desc';

export interface MovieSearchRequest {
  title?: string;
  year?: number | null;
  genre?: string;
  orderBy?: MovieOrderBy;
  orderDirection?: OrderDirection;
  cursor?: string;
}
