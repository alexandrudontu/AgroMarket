import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ProductsService } from '../products.service';
import { CartService } from '../../cart/cart.service';
import { AuthService } from '../../auth/auth.service';
import { ChangeDetectorRef } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.html',
  standalone: true,
  styleUrls: ['./product-details.css'],
  imports: [CommonModule]
})
export class ProductDetailsComponent implements OnInit {

  product: any;
  selectedImage: any = null;
  quantity = 1;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductsService,
    private cartService: CartService,
    public authService: AuthService,
    private cdr: ChangeDetectorRef,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id') ?? '0');

    this.productService.getById(id).subscribe((res: any) => {
      this.product = res;
      console.log(this.product);

      if (this.product.images?.length) {
        this.selectedImage = this.product.images[0].imageUrl;
      }
      this.cdr.detectChanges();
    });
  }

  addToCart() {
    this.cartService.addToCart({
      productId: this.product.id,
      quantity: this.quantity
    }).subscribe(() => {
      this.toastr.success('Produs adăugat în coș!');
    });
  }

  increaseQty() {
    if (this.quantity < this.product.quantity) {

      this.quantity++;
    }
  }

  decreaseQty() {
    if (this.quantity > 1) {

      this.quantity--;
    }
  }
}
