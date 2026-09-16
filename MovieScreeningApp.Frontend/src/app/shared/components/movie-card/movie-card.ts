import { Component, input, output } from '@angular/core';
import { MovieSummary } from '../../models/movie-summary';

@Component({
  imports: [],
  selector: 'app-movie-card',
  styleUrl: './movie-card.scss',
  templateUrl: './movie-card.html',
})
export class MovieCard {
  readonly movie = input.required<MovieSummary>();
  readonly isFavorite = input(false);
  readonly favoriteToggle = output<MovieSummary>();
}
