import { Component, inject, OnInit, signal } from '@angular/core';
import { FavoriteService } from '../../services/favorite.service';
import { MovieCard } from '../../../shared/components/movie-card/movie-card';
import { Favorite } from '../../models/favorite';
import { MovieSummary } from '../../../shared/models/movie-summary';
import { getErrorMessage } from '../../../shared/http-error';

@Component({
  imports: [MovieCard],
  selector: 'app-favorite-list',
  styleUrl: './favorite-list.scss',
  templateUrl: './favorite-list.html',
})
export class FavoriteList implements OnInit {
  private readonly favoriteService = inject(FavoriteService);

  readonly favorites = signal<Favorite[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.favoriteService.getAll().subscribe({
      next: favorites => {
        this.favorites.set(favorites);
        this.loading.set(false);
      },
      error: e => {
        this.error.set(getErrorMessage(e));
        this.loading.set(false);
      }
    });
  }

  remove(movie: MovieSummary): void {
    this.favoriteService.remove(movie.showId).subscribe({
      next: () => this.favorites.update(favorites => favorites.filter(f => f.showId !== movie.showId)),
      error: e => this.error.set(getErrorMessage(e))
    });
  }
}
