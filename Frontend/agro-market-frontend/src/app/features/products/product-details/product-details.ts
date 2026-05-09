import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ProductsService } from '../products.service';
import { CartService } from '../../cart/cart.service';
import { AuthService } from '../../auth/auth.service';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.html',
  standalone: true,
  imports: [CommonModule]
})
export class ProductDetailsComponent implements OnInit {

  product: any;
  selectedImage: string = '';

  constructor(
    private route: ActivatedRoute,
    private productService: ProductsService,
    private cartService: CartService,
    public authService: AuthService,
    private cdr: ChangeDetectorRef
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
      quantity: 1
    }).subscribe(() => alert('Added to cart'));
  }
}
