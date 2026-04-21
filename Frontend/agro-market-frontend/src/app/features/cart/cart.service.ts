import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class CartService {

  private baseUrl = 'https://localhost:7183/api/cart';

  constructor(private http: HttpClient) {}

  getCart() {
    return this.http.get(this.baseUrl);
  }

  addToCart(item: any) {
    return this.http.post(this.baseUrl, item);
  }

  remove(productId: number) {
    return this.http.delete(`${this.baseUrl}/${productId}`);
  }

  clear() {
    return this.http.delete(`${this.baseUrl}/clear`);
  }
}
