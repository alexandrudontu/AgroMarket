import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { FarmersPublicService } from '../farmers-public.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-farmers-nearby',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './farmers-nearby.component.html',
  styleUrls: ['./farmers-nearby.component.css']
})
export class FarmersNearbyComponent {

  farmers: any[] = [];

  latitude?: number;
  longitude?: number;

  maxDistanceKm = 50;
  searched = false;
  isLoading = false;

  constructor(
    private farmersService: FarmersPublicService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  useMyLocation() {

    if (!navigator.geolocation) {
      this.toastr.error('Browserul nu suportă geolocația.');
      return;
    }

    this.isLoading = true;

    navigator.geolocation.getCurrentPosition(
      position => {

        this.latitude = position.coords.latitude;
        this.longitude = position.coords.longitude;

        this.loadNearbyFarmers();
      },
      error => {
        console.error(error);

        this.isLoading = false;
        this.toastr.warning('Accesul la locație a fost refuzat.');
        this.cdr.detectChanges();
      }
    );
  }

  loadNearbyFarmers() {

    if (this.latitude == null || this.longitude == null) {
      this.toastr.warning('Selectează locația mai întâi.');
      return;
    }

    this.isLoading = true;

    this.farmersService.getNearbyFarmers(
      this.latitude,
      this.longitude,
      this.maxDistanceKm
    ).subscribe({
      next: res => {
        this.farmers = res;
        this.searched = true;
        this.isLoading = false;

        if (!res.length) {
          this.toastr.info('Nu există fermieri în apropierea ta.');
        }

        this.cdr.detectChanges();
      },
      error: err => {
        console.error(err);

        this.isLoading = false;
        this.toastr.error('Nu am putut încărca fermierii.');
        this.cdr.detectChanges();
      }
    });
  }
}