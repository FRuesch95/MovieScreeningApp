import { inject, Service } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Genre } from '../models/genre';
import { MovieSearchRequest } from '../models/movie-search-request';
import { MovieSearchResponse } from '../models/movie-search-response';

@Service()
export class MovieService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/StreamingAvailability';

  searchMovies(request: MovieSearchRequest): Observable<MovieSearchResponse> {
    let params = new HttpParams();

    for (const [key, value] of Object.entries(request)) {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, String(value));
      }
    }

    return this.http.get<MovieSearchResponse>(`${this.baseUrl}/movies`, { params });
  }

  getGenres(): Observable<Genre[]> {
    return this.http.get<Genre[]>(`${this.baseUrl}/genres`);
  }
}
