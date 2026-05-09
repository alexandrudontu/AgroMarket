import { HttpClient, HttpParams } from '@angular/common/http';
import co from '@angular/common/locales/co';
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

  getMyProducts() {
    return this.http.get<any[]>(`${this.baseUrl}/my`);
  }

  create(product: any) {
    return this.http.post(this.baseUrl, product);
  }

  update(id: number, product: any) {
    return this.http.put(
      `${this.baseUrl}/${id}`,
      product
    );
  }

  delete(id: number) {
    return this.http.delete(
      `${this.baseUrl}/${id}`
    );
  }
  
  uploadImage(productId: number, file: File) {

    const formData = new FormData();

    formData.append('productId', productId.toString());
    formData.append('file', file);

    return this.http.post(
      `${this.baseUrl}/${productId}/images`,
      formData
    );
  }
  deleteImage(id: number) {
    return this.http.delete(
      `${this.baseUrl}/images/${id}`
    );
  }
}
