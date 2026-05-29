import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class FarmersPublicService {

  private baseUrl = 'https://localhost:7183/api/farmers';

  constructor(private http: HttpClient) {}

  getNearbyFarmers(
    latitude: number,
    longitude: number,
    maxDistanceKm: number = 50
  ) {
    return this.http.get<any[]>(
      `${this.baseUrl}/nearby`,
      {
        params: {
          latitude,
          longitude,
          maxDistanceKm
        }
      }
    );
  }

  getFarmerProfile(
    id: string,
    latitude?: number,
    longitude?: number
  ) {
    const params: any = {};

    if (latitude != null) {
      params.latitude = latitude;
    }

    if (longitude != null) {
      params.longitude = longitude;
    }

    return this.http.get<any>(
      `${this.baseUrl}/${id}`,
      { params }
    );
  }
}