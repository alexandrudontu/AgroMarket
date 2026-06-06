import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class FarmersService {

  private baseUrl = `${environment.apiUrl}/api/users/farmers`;

  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get(this.baseUrl);
  }

  getById(id: number) {
    return this.http.get(`${this.baseUrl}/${id}`);
  }

  getFarmerOrders(farmerId: string) {
    return this.http.get(
      `${environment.apiUrl}/api/orders/farmer/${farmerId}`
    );
  }
}
