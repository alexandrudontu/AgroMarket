import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FarmersPublicService } from '../farmers-public.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-farmer-profile',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './farmer-profile.component.html',
  styleUrls: ['./farmer-profile.component.css']
})
export class FarmerProfileComponent implements OnInit {

  farmer: any;
  isLoading = false;

  latitude?: number;
  longitude?: number;

  constructor(
    private route: ActivatedRoute,
    private farmersService: FarmersPublicService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');

    this.latitude = this.route.snapshot.queryParamMap.get('lat')
      ? Number(this.route.snapshot.queryParamMap.get('lat'))
      : undefined;

    this.longitude = this.route.snapshot.queryParamMap.get('lng')
      ? Number(this.route.snapshot.queryParamMap.get('lng'))
      : undefined;

    if (!id) {
      this.toastr.error('Fermier invalid.');
      return;
    }

    this.loadFarmer(id);
  }

  loadFarmer(id: string) {

    this.isLoading = true;

    this.farmersService.getFarmerProfile(
      id,
      this.latitude,
      this.longitude
    ).subscribe({
      next: res => {
        this.farmer = res;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: err => {
        console.error(err);
        this.isLoading = false;
        this.toastr.error('Profilul fermierului nu a putut fi încărcat.');
        this.cdr.detectChanges();
      }
    });
  }

  getMainImage(product: any): string | null {
    if (!product.productImages?.length) {
      return null;
    }

    return 'https://localhost:7183' + product.productImages[0].imageUrl;
  }
}