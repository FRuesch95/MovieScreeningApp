import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { MovieService } from './movie.service';

describe('MovieService', () => {
  let service: MovieService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(MovieService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should send only set filters as query params', () => {
    service.searchMovies({ title: 'dune', year: 2021, orderBy: 'rating', orderDirection: 'desc' }).subscribe();

    const req = httpMock.expectOne(r => r.url === '/api/StreamingAvailability/movies');
    expect(req.request.params.get('title')).toBe('dune');
    expect(req.request.params.get('year')).toBe('2021');
    expect(req.request.params.get('orderBy')).toBe('rating');
    expect(req.request.params.get('orderDirection')).toBe('desc');
    expect(req.request.params.has('genre')).toBe(false);
    expect(req.request.params.has('cursor')).toBe(false);
    req.flush({ shows: [], hasMore: false, nextCursor: null });
  });

  it('should load genres', () => {
    service.getGenres().subscribe(genres => expect(genres.length).toBe(1));

    const req = httpMock.expectOne('/api/StreamingAvailability/genres');
    expect(req.request.method).toBe('GET');
    req.flush([{ id: 'action', name: 'Action' }]);
  });
});
