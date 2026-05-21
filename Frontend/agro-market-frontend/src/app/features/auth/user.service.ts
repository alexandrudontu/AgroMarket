import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient) { }

updateFarmerLocation(data: any) {
  return this.http.put(
    'https://localhost:7183/api/users/farmer/location',
    data
  );
}

}
