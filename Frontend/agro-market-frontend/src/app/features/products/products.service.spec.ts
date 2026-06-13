import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../environments/environment';
import { ProductsService } from './products.service';

describe('ProductsService', () => {
  let service: ProductsService;
  let httpMock: HttpTestingController;
  const baseUrl = `${environment.apiUrl}/api/products`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(ProductsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('sends only non-empty filter parameters when requesting products', () => {
    service.getAll({
      search: 'rosii',
      categoryId: 2,
      minPrice: '',
      maxPrice: null
    }).subscribe(products => {
      expect(products).toEqual([]);
    });

    const request = httpMock.expectOne(req =>
      req.method === 'GET' &&
      req.url === baseUrl &&
      req.params.get('search') === 'rosii' &&
      req.params.get('categoryId') === '2' &&
      !req.params.has('minPrice') &&
      !req.params.has('maxPrice')
    );

    request.flush([]);
  });

  it('requests one product by id', () => {
    service.getById(7).subscribe(product => {
      expect(product).toEqual({ id: 7, name: 'Mere' });
    });

    const request = httpMock.expectOne(`${baseUrl}/7`);
    expect(request.request.method).toBe('GET');

    request.flush({ id: 7, name: 'Mere' });
  });

  it('posts product data when creating a product', () => {
    const product = {
      name: 'Mere',
      price: 5,
      quantity: 10,
      unitOfMeasurement: 'kg',
      categoryId: 1
    };

    service.create(product).subscribe(response => {
      expect(response).toEqual({ id: 1, ...product });
    });

    const request = httpMock.expectOne(baseUrl);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(product);

    request.flush({ id: 1, ...product });
  });

  it('updates an existing product', () => {
    const product = { name: 'Pere', price: 8 };

    service.update(3, product).subscribe(response => {
      expect(response).toEqual({ id: 3, ...product });
    });

    const request = httpMock.expectOne(`${baseUrl}/3`);
    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual(product);

    request.flush({ id: 3, ...product });
  });

  it('deletes a product', () => {
    service.delete(4).subscribe(response => {
      expect(response).toEqual({});
    });

    const request = httpMock.expectOne(`${baseUrl}/4`);
    expect(request.request.method).toBe('DELETE');

    request.flush({});
  });

  it('uploads a product image as FormData', () => {
    const file = new File(['image-content'], 'produs.jpg', { type: 'image/jpeg' });

    service.uploadImage(10, file).subscribe(response => {
      expect(response).toEqual({ id: 1, imageUrl: 'produs.jpg' });
    });

    const request = httpMock.expectOne(`${baseUrl}/10/images`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body instanceof FormData).toBe(true);
    expect(request.request.body.get('productId')).toBe('10');
    expect(request.request.body.get('file')).toBe(file);

    request.flush({ id: 1, imageUrl: 'produs.jpg' });
  });

  it('requests nearby products with location parameters', () => {
    service.getNearbyProducts(44.31, 23.79, 25, 2, 1, 20).subscribe(products => {
      expect(products).toEqual([]);
    });

    const request = httpMock.expectOne(req =>
      req.method === 'GET' &&
      req.url === `${baseUrl}/nearby` &&
      req.params.get('latitude') === '44.31' &&
      req.params.get('longitude') === '23.79' &&
      req.params.get('maxDistanceKm') === '25' &&
      req.params.get('categoryId') === '2' &&
      req.params.get('minPrice') === '1' &&
      req.params.get('maxPrice') === '20'
    );

    request.flush([]);
  });
});
