import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CartService {

  private baseUrl = `${environment.apiUrl}/api/cart`;

  constructor(private http: HttpClient) {}

  getCart() {
    return this.http.get(this.baseUrl);
  }

  addToCart(item: any) {
    return this.http.post(this.baseUrl, item);
  }

  updateQuantity(data: any) {    
    return this.http.put(
      `${this.baseUrl}/update`,
      data
    );
  }

  remove(productId: number) {
    return this.http.delete(`${this.baseUrl}/${productId}`);
  }

  clear() {
    return this.http.delete(`${this.baseUrl}/clear`);
  }
}
