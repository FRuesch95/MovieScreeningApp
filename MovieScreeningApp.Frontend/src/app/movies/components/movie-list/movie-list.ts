import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MovieService } from '../../services/movie.service';
import { FavoriteService } from '../../../favorites/services/favorite.service';
import { MovieCard } from '../../../shared/components/movie-card/movie-card';
import { Genre } from '../../models/genre';
import { MovieOrderBy, MovieSearchRequest } from '../../models/movie-search-request';
import { toMovieSummary } from '../../models/movie';
import { MovieSummary } from '../../../shared/models/movie-summary';
import { getErrorMessage } from '../../../shared/http-error';

@Component({
  imports: [FormsModule, MovieCard],
  selector: 'app-movie-list',
  styleUrl: './movie-list.scss',
  templateUrl: './movie-list.html',
})
export class MovieList implements OnInit {
  private readonly movieService = inject(MovieService);
  private readonly favoriteService = inject(FavoriteService);

  readonly sortOptions: { value: MovieOrderBy; label: string }[] = [
    { value: 'popularity_alltime', label: 'Popularity' },
    { value: 'rating', label: 'Rating' },
    { value: 'release_date', label: 'Release date' },
    { value: 'original_title', label: 'Title' }
  ];

  filter: MovieSearchRequest = {
    title: '',
    year: null,
    genre: '',
    orderBy: 'popularity_alltime',
    orderDirection: 'desc'
  };

  readonly genres = signal<Genre[]>([]);
  readonly movies = signal<MovieSummary[]>([]);
  readonly favoriteIds = signal<Set<string>>(new Set());
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly page = signal(1);
  readonly hasNextPage = signal(false);
  private previousCursors: (string | undefined)[] = [];
  private currentCursor: string | undefined;
  private nextCursor: string | null = null;

  ngOnInit(): void {
    this.movieService.getGenres().subscribe(genres => this.genres.set(genres));
    this.favoriteService.getAll().subscribe(favorites =>
      this.favoriteIds.set(new Set(favorites.map(f => f.showId)))
    );
    this.search();
  }

  search(): void {
    this.previousCursors = [];
    this.loadPage(undefined);
  }

  toggleSortDirection(): void {
    this.filter.orderDirection = this.filter.orderDirection === 'asc' ? 'desc' : 'asc';
    this.search();
  }

  nextPage(): void {
    if (!this.nextCursor) {
      return;
    }

    this.previousCursors.push(this.currentCursor);
    this.loadPage(this.nextCursor);
  }

  previousPage(): void {
    this.loadPage(this.previousCursors.pop());
  }

  toggleFavorite(movie: MovieSummary): void {
    if (this.favoriteIds().has(movie.showId)) {
      this.favoriteService.remove(movie.showId).subscribe({
        next: () => this.favoriteIds.update(ids => {
          const updated = new Set(ids);
          updated.delete(movie.showId);
          return updated;
        }),
        error: e => this.error.set(getErrorMessage(e))
      });
    } else {
      this.favoriteService.add(movie).subscribe({
        next: () => this.favoriteIds.update(ids => new Set(ids).add(movie.showId)),
        error: e => this.error.set(getErrorMessage(e))
      });
    }
  }

  private loadPage(cursor: string | undefined): void {
    this.loading.set(true);
    this.error.set(null);

    this.movieService.searchMovies({ ...this.filter, cursor }).subscribe({
      next: response => {
        this.movies.set(response.shows.map(toMovieSummary));
        this.currentCursor = cursor;
        this.nextCursor = response.nextCursor;
        this.hasNextPage.set(response.hasMore);
        this.page.set(this.previousCursors.length + 1);
        this.loading.set(false);
      },
      error: e => {
        this.error.set(getErrorMessage(e));
        this.loading.set(false);
      }
    });
  }
}
