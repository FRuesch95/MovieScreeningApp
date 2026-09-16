import { Genre } from './genre';
import { MovieSummary } from '../../shared/models/movie-summary';

export interface VerticalPoster {
  w240: string | null;
  w360: string | null;
  w480: string | null;
  w600: string | null;
  w720: string | null;
}

export interface MovieImageSet {
  verticalPoster: VerticalPoster;
}

export interface Movie {
  id: string;
  title: string;
  overview: string;
  releaseYear: number | null;
  genres: Genre[];
  rating: number;
  imageSet: MovieImageSet;
}

export function toMovieSummary(movie: Movie): MovieSummary {
  return {
    showId: movie.id,
    title: movie.title,
    overview: movie.overview,
    releaseYear: movie.releaseYear,
    rating: movie.rating,
    genres: movie.genres.map(g => g.name).join(', '),
    posterUrl: movie.imageSet.verticalPoster.w360
  };
}
