import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class FarmersService {

  private baseUrl = 'https://localhost:7183/api/users/farmers';

  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get(this.baseUrl);
  }

  getById(id: number) {
    return this.http.get(`${this.baseUrl}/${id}`);
  }

  getFarmerOrders(farmerId: string) {
    return this.http.get(
      `https://localhost:7183/api/orders/farmer/${farmerId}`
    );
  }
}
