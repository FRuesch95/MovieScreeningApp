import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./home/home')
        .then(m => m.Home)
  },
  {
    path: 'movies',
    loadComponent: () =>
      import('./movies/components/movie-list/movie-list')
        .then(m => m.MovieList)
  },
  {
    path: 'not-implemented-yet',
    loadComponent: () =>
      import('./shared/components/not-implemented-yet/not-implemented-yet')
        .then(m => m.NotImplementedYet)
  }
];
