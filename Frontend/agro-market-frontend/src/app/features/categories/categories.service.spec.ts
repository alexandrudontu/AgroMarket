import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../environments/environment';
import { CategoriesService } from './categories.service';

describe('CategoriesService', () => {
  let service: CategoriesService;
  let httpMock: HttpTestingController;
  const baseUrl = `${environment.apiUrl}/api/categories`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(CategoriesService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('requests all categories', () => {
    const categories = [
      { id: 1, name: 'Fructe', icon: '🍎' },
      { id: 2, name: 'Legume', icon: '🥕' }
    ];

    service.getAll().subscribe(response => {
      expect(response).toEqual(categories);
    });

    const request = httpMock.expectOne(baseUrl);
    expect(request.request.method).toBe('GET');

    request.flush(categories);
  });

  it('creates a category', () => {
    const category = { name: 'Lactate', icon: '🥛' };

    service.create(category).subscribe(response => {
      expect(response).toEqual({ id: 3, ...category });
    });

    const request = httpMock.expectOne(baseUrl);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(category);

    request.flush({ id: 3, ...category });
  });

  it('deletes a category', () => {
    service.delete(4).subscribe(response => {
      expect(response).toEqual({});
    });

    const request = httpMock.expectOne(`${baseUrl}/4`);
    expect(request.request.method).toBe('DELETE');

    request.flush({});
  });
});
