import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductsService } from '../../products/products.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-category-products',
  standalone: true,
  templateUrl: './category-products.html',
  styleUrls: ['./category-products.css'],
  imports: [RouterModule, CommonModule, FormsModule]
})
export class CategoryProductsComponent implements OnInit {

  products: any[] = [];

  categoryId!: number;

  minPrice?: number;
  maxPrice?: number;

  latitude?: number;
  longitude?: number;

  useLocationFilter = false;
  maxDistanceKm = 5000;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductsService,
    private cdr: ChangeDetectorRef,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.categoryId = Number(this.route.snapshot.paramMap.get('id'));

    this.route.queryParams.subscribe(params => {

      this.minPrice = params['minPrice'] !== undefined
        ? Number(params['minPrice'])
        : undefined;

      this.maxPrice = params['maxPrice'] !== undefined
        ? Number(params['maxPrice'])
        : undefined;

      this.useLocationFilter = params['location'] === 'true';

      this.latitude = params['lat'] !== undefined
        ? Number(params['lat'])
        : undefined;

      this.longitude = params['lng'] !== undefined
        ? Number(params['lng'])
        : undefined;

      this.maxDistanceKm = params['maxDistance'] !== undefined
        ? Number(params['maxDistance'])
        : 500;

      if (
        this.useLocationFilter &&
        this.latitude !== undefined &&
        this.longitude !== undefined
      ) {
        this.loadNearbyProducts();
      } else {
        this.loadProducts();
      }
    });
  }

  loadProducts() {
    this.productService.getAll({
      categoryId: this.categoryId,
      minPrice: this.minPrice,
      maxPrice: this.maxPrice
    }).subscribe({
      next: (res: any[]) => {
        this.products = res;
        this.cdr.detectChanges();
      },
      error: err => {
        console.error(err);
        this.toastr.error('Produsele nu au putut fi încărcate.');
      }
    });
  }

  loadNearbyProducts() {

    if (this.latitude === undefined || this.longitude === undefined) {
      this.toastr.warning('Selectează mai întâi locația.');
      return;
    }

    this.productService.getNearbyProducts(
      this.latitude,
      this.longitude,
      this.maxDistanceKm,
      this.categoryId,
      this.minPrice,
      this.maxPrice
    ).subscribe({
      next: (res: any[]) => {

        this.products = res;

        if (!res.length) {
          this.toastr.info('Nu există produse apropiate pentru filtrele alese.');
        }

        this.cdr.detectChanges();
      },
      error: err => {
        console.error(err);
        this.toastr.error('Nu am putut încărca produsele apropiate.');
      }
    });
  }

  applyFilters() {

    if (this.minPrice != null && this.minPrice < 0) {
      this.minPrice = 0;
    }

    if (this.maxPrice != null && this.maxPrice < 0) {
      this.maxPrice = 0;
    }

    if (
      this.minPrice != null &&
      this.maxPrice != null &&
      this.minPrice > this.maxPrice
    ) {
      this.toastr.warning('Prețul minim nu poate fi mai mare decât prețul maxim.');
      return;
    }

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        minPrice: this.minPrice ?? null,
        maxPrice: this.maxPrice ?? null,
        location: this.useLocationFilter ? 'true' : null,
        lat: this.useLocationFilter ? this.latitude : null,
        lng: this.useLocationFilter ? this.longitude : null,
        maxDistance: this.useLocationFilter ? this.maxDistanceKm : null
      },
      queryParamsHandling: 'merge'
    });

    if (this.useLocationFilter) {
      this.loadNearbyProducts();
    } else {
      this.loadProducts();
    }
  }

  useMyLocation() {

    if (!navigator.geolocation) {
      this.toastr.error('Browserul nu suportă geolocația.');
      return;
    }

    navigator.geolocation.getCurrentPosition(
      position => {

        this.latitude = position.coords.latitude;
        this.longitude = position.coords.longitude;
        this.useLocationFilter = true;

        this.router.navigate([], {
          relativeTo: this.route,
          queryParams: {
            location: 'true',
            lat: this.latitude,
            lng: this.longitude,
            maxDistance: this.maxDistanceKm,
            minPrice: this.minPrice ?? null,
            maxPrice: this.maxPrice ?? null
          },
          queryParamsHandling: 'merge'
        });

        this.loadNearbyProducts();
      },
      error => {
        console.error(error);
        this.toastr.warning('Accesul la locație a fost refuzat.');
      }
    );
  }

  clearLocationFilter() {

    this.useLocationFilter = false;
    this.latitude = undefined;
    this.longitude = undefined;

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        location: null,
        lat: null,
        lng: null,
        maxDistance: null
      },
      queryParamsHandling: 'merge'
    });

    this.loadProducts();
  }
}