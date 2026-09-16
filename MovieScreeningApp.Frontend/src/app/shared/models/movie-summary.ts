export interface MovieSummary {
  showId: string;
  title: string;
  overview: string | null;
  releaseYear: number | null;
  rating: number;
  genres: string | null;
  posterUrl: string | null;
}
