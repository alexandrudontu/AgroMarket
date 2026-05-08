import { Component, OnInit } from '@angular/core';
import { ProductsService } from '../../products/products.service';
import { OrdersService } from '../../orders/orders.service';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-farmer-dashboard',
  templateUrl: './dashboard.html'
})
export class FarmerDashboardComponent implements OnInit {

  products: any[] = [];
  orders: any[] = [];

  constructor(
    private productService: ProductsService,
    private orderService: OrdersService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadProducts();
    this.loadOrders();
  }

  loadProducts() {
    this.productService.getMyProducts().subscribe((res: any) => {
      this.products = res;
      this.cdr.detectChanges();
    });
  }

  loadOrders() {
    this.orderService.getFarmerOrders().subscribe((res: any) => {
      this.orders = res;
      this.cdr.detectChanges();
    });
  }
}
