import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { FavoriteList } from './favorite-list';

describe('FavoriteList', () => {
  let component: FavoriteList;
  let fixture: ComponentFixture<FavoriteList>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FavoriteList],
      providers: [provideHttpClient(), provideHttpClientTesting()]
    }).compileComponents();

    fixture = TestBed.createComponent(FavoriteList);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    await fixture.whenStable();
  });

  afterEach(() => httpMock.verify());

  it('should remove a favourite from the list', () => {
    const favorite = { id: 1, showId: '123', title: 'Dune', overview: null, releaseYear: 2021, rating: 78, genres: null, posterUrl: null };
    httpMock.expectOne('/api/Favorites').flush([favorite]);
    expect(component.favorites().length).toBe(1);

    component.remove(favorite);
    httpMock.expectOne('/api/Favorites/123').flush(null);

    expect(component.favorites().length).toBe(0);
  });
});
