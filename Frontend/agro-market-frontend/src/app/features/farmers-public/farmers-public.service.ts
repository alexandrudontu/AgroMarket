import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FarmersPublicService {

  private baseUrl = `${environment.apiUrl}/api/farmers`;

  constructor(private http: HttpClient) {}

  getNearbyFarmers(
    latitude: number,
    longitude: number,
    maxDistanceKm: number = 500
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