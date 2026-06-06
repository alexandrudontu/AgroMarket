import { HttpClient, HttpParams } from '@angular/common/http';
import co from '@angular/common/locales/co';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ProductsService {

  private baseUrl = `${environment.apiUrl}/api/products`;

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

  getNearbyProducts(
    latitude: number,
    longitude: number,
    maxDistanceKm: number = 5000,
    categoryId?: number,
    minPrice?: number,
    maxPrice?: number
  ) {
    const params: any = {
      latitude: latitude,
      longitude: longitude,
      maxDistanceKm: maxDistanceKm
    };
  
    if (categoryId != null) {
      params.categoryId = categoryId;
    }
  
    if (minPrice != null) {
      params.minPrice = minPrice;
    }
  
    if (maxPrice != null) {
      params.maxPrice = maxPrice;
    }
  
    return this.http.get<any[]>(
      `${this.baseUrl}/nearby`,
      { params: params }
    );
  }
}
