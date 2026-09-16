import { MovieSummary } from '../../shared/models/movie-summary';

export interface Favorite extends MovieSummary {
  id: number;
}
