import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { MovieList } from './movie-list';

describe('MovieList', () => {
  let component: MovieList;
  let fixture: ComponentFixture<MovieList>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MovieList],
      providers: [provideHttpClient(), provideHttpClientTesting()]
    }).compileComponents();

    fixture = TestBed.createComponent(MovieList);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    await fixture.whenStable();
  });

  afterEach(() => httpMock.verify());

  it('should load genres, favourites and the first page on init', () => {
    httpMock.expectOne('/api/StreamingAvailability/genres').flush([]);
    httpMock.expectOne('/api/Favorites').flush([]);

    const search = httpMock.expectOne(r => r.url === '/api/StreamingAvailability/movies');
    expect(search.request.params.get('orderBy')).toBe('popularity_alltime');
    expect(search.request.params.has('cursor')).toBe(false);
    search.flush({ shows: [], hasMore: false, nextCursor: null });

    expect(component.page()).toBe(1);
    expect(component.hasNextPage()).toBe(false);
  });

  it('should move to page 2 and back to page 1', () => {
    httpMock.expectOne('/api/StreamingAvailability/genres').flush([]);
    httpMock.expectOne('/api/Favorites').flush([]);
    httpMock.expectOne(r => r.url === '/api/StreamingAvailability/movies')
      .flush({ shows: [], hasMore: true, nextCursor: 'cursor-2' });

    component.nextPage();
    const secondPage = httpMock.expectOne(r => r.url === '/api/StreamingAvailability/movies');
    expect(secondPage.request.params.get('cursor')).toBe('cursor-2');
    secondPage.flush({ shows: [], hasMore: false, nextCursor: null });
    expect(component.page()).toBe(2);

    component.previousPage();
    const firstPage = httpMock.expectOne(r => r.url === '/api/StreamingAvailability/movies');
    expect(firstPage.request.params.has('cursor')).toBe(false);
    firstPage.flush({ shows: [], hasMore: true, nextCursor: 'cursor-2' });
    expect(component.page()).toBe(1);
  });
});
