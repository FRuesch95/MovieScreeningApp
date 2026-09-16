import { inject, Service } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Favorite } from '../models/favorite';
import { MovieSummary } from '../../shared/models/movie-summary';

@Service()
export class FavoriteService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/Favorites';

  getAll(): Observable<Favorite[]> {
    return this.http.get<Favorite[]>(this.baseUrl);
  }

  add(movie: MovieSummary): Observable<Favorite> {
    return this.http.post<Favorite>(this.baseUrl, movie);
  }

  remove(showId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${encodeURIComponent(showId)}`);
  }
}
