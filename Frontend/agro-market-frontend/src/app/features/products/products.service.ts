import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ProductsService {

  private baseUrl = 'https://localhost:7183/api/products';

  constructor(private http: HttpClient) {}

  getAll(params?: any) {
    return this.http.get(this.baseUrl, { params });
  }

  getById(id: number) {
    return this.http.get(`${this.baseUrl}/${id}`);
  }

  create(product: any) {
    return this.http.post(this.baseUrl, product);
  }

  getMyProducts() {
    return this.http.get(`${this.baseUrl}/my`);
  }
}
