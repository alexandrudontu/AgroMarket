import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../environments/environment';
import { OrdersService } from './orders.service';

describe('OrdersService', () => {
  let service: OrdersService;
  let httpMock: HttpTestingController;
  const baseUrl = `${environment.apiUrl}/api/orders`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(OrdersService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('sends checkout request', () => {
    service.checkout().subscribe(response => {
      expect(response).toEqual({ orderId: 1 });
    });

    const request = httpMock.expectOne(`${baseUrl}/checkout`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({});

    request.flush({ orderId: 1 });
  });

  it('requests farmer orders', () => {
    const orders = [
      { id: 1, customerName: 'Alex Popescu', totalAmount: 20 }
    ];

    service.getFarmerOrders().subscribe(response => {
      expect(response).toEqual(orders);
    });

    const request = httpMock.expectOne(`${baseUrl}/farmer`);
    expect(request.request.method).toBe('GET');

    request.flush(orders);
  });
});
