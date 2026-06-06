import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class OrdersService {

  private baseUrl = `${environment.apiUrl}/api/orders`;

  constructor(private http: HttpClient) {}

  checkout() {
    return this.http.post(`${this.baseUrl}/checkout`, {});
  }

  getFarmerOrders() {
    return this.http.get(`${this.baseUrl}/farmer`);
  }
}
