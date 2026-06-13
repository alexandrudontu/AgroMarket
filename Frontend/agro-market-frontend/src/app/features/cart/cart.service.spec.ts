import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../environments/environment';
import { CartService } from './cart.service';

describe('CartService', () => {
  let service: CartService;
  let httpMock: HttpTestingController;
  const baseUrl = `${environment.apiUrl}/api/cart`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(CartService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('requests the current cart', () => {
    const mockCart = {
      items: [
        { productId: 1, productName: 'Mere', quantity: 2, unitPrice: 5 }
      ],
      totalAmount: 10
    };

    service.getCart().subscribe(cart => {
      expect(cart).toEqual(mockCart);
    });

    const request = httpMock.expectOne(baseUrl);
    expect(request.request.method).toBe('GET');

    request.flush(mockCart);
  });

  it('adds a product to the cart', () => {
    const item = { productId: 3, quantity: 2 };

    service.addToCart(item).subscribe(response => {
      expect(response).toEqual({ success: true });
    });

    const request = httpMock.expectOne(baseUrl);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(item);

    request.flush({ success: true });
  });

  it('updates product quantity using the update endpoint', () => {
    const data = { productId: 3, quantity: 5 };

    service.updateQuantity(data).subscribe(response => {
      expect(response).toEqual({ success: true });
    });

    const request = httpMock.expectOne(`${baseUrl}/update`);
    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual(data);

    request.flush({ success: true });
  });

  it('removes one product from the cart', () => {
    service.remove(8).subscribe(response => {
      expect(response).toEqual({ success: true });
    });

    const request = httpMock.expectOne(`${baseUrl}/8`);
    expect(request.request.method).toBe('DELETE');

    request.flush({ success: true });
  });

  it('clears the cart', () => {
    service.clear().subscribe(response => {
      expect(response).toEqual({ success: true });
    });

    const request = httpMock.expectOne(`${baseUrl}/clear`);
    expect(request.request.method).toBe('DELETE');

    request.flush({ success: true });
  });
});
