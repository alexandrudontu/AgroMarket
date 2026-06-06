import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient) { }

updateFarmerLocation(data: any) {
  return this.http.put(
    `${environment.apiUrl}/api/users/farmer/location`,
    data
  );
}

}
