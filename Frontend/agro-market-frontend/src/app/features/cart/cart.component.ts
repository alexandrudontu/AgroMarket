import { Component, OnInit } from '@angular/core';
import { CartService } from './cart.service';
import { OrdersService } from '.././orders/orders.service';
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { RouterModule } from '@angular/router';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css'],
  imports: [CommonModule, RouterModule]
})
export class CartComponent implements OnInit {

  cart: any;
  apiUrl = environment.apiUrl;

  constructor(
    private cartService: CartService,
    private orderService: OrdersService,
    private cdr: ChangeDetectorRef,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadCart();
  }

  loadCart() {
    this.cartService.getCart().subscribe((res: any) => {
      this.cart = res;
      this.cdr.detectChanges();
    });
  }

  remove(productId: number) {
    this.cartService.remove(productId).subscribe(() => {
      this.loadCart();
      this.toastr.warning('Produs eliminat din coș.');
      this.cdr.detectChanges();
    });
  }

  clear() {
    this.cartService.clear().subscribe(() => {
      this.loadCart();
      this.toastr.success('Coșul a fost golit.');
      this.cdr.detectChanges();
    });
  }

  checkout() {
    this.orderService.checkout().subscribe({
      next: (res: any) => {
        this.toastr.success('Comanda a fost plasată!');
        this.loadCart();
        this.cdr.detectChanges();
      },
      error: () => {
        this.toastr.error('Plasarea comenzii a eșuat.');
      }
    });
  }

  increaseQuantity(item: any) {
    item.quantity++;

    this.updateQuantity(item);
  }

  decreaseQuantity(item: any) {
    if (item.quantity <= 1)
      return;

    item.quantity--;

    this.updateQuantity(item);
  }

  updateQuantity(item: any) {
    this.cartService.updateQuantity({
      productId: item.productId,
      quantity: item.quantity
    }).subscribe(() => {

      this.loadCart();
    });
  }
}
