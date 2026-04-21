import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class OrdersService {

  private baseUrl = 'https://localhost:7183/api/orders';

  constructor(private http: HttpClient) {}

  checkout() {
    return this.http.post(`${this.baseUrl}/checkout`, {});
  }

  getFarmerOrders() {
    return this.http.get(`${this.baseUrl}/farmer`);
  }
}
