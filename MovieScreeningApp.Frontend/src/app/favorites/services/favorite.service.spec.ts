import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { FavoriteService } from './favorite.service';

describe('FavoriteService', () => {
  let service: FavoriteService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(FavoriteService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should post a favorite', () => {
    const movie = { showId: '123', title: 'Dune', overview: null, releaseYear: 2021, rating: 78, genres: null, posterUrl: null };
    service.add(movie).subscribe();

    const req = httpMock.expectOne('/api/Favorites');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(movie);
    req.flush({ id: 1, ...movie });
  });

  it('should delete a favorite by show id', () => {
    service.remove('123').subscribe();

    const req = httpMock.expectOne('/api/Favorites/123');
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
