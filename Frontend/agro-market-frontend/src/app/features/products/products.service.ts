import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ProductsService {

  private baseUrl = 'https://localhost:7183/api/products';

  constructor(private http: HttpClient) {}

  getAll(paramsObj?: any) {
    let params = new HttpParams();

    if (paramsObj) {
      Object.keys(paramsObj).forEach(key => {
        if (paramsObj[key] !== null && paramsObj[key] !== undefined && paramsObj[key] !== '') {
          params = params.append(key, paramsObj[key]);
        }
      });
    }

    return this.http.get<any[]>(this.baseUrl, { params });
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
