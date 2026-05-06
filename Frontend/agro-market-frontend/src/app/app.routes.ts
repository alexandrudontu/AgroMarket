import { Routes } from '@angular/router';
import { ProductsList } from './features/products/products-list/products-list';
import { ProductDetailsComponent } from './features/products/product-details/product-details';
import { LoginComponent } from './features/auth/login/login';
import { RegisterComponent } from './features/auth/register/register';
import { CartComponent } from './features/cart/cart.component';
import { OrdersComponent } from './features/orders/orders.component';
import { FarmerDashboardComponent } from './features/farmer/dashboard/dashboard';
import { authGuard } from './core/guards/auth-guard';
import { roleGuard } from './core/guards/role-guard';

export const routes: Routes = [
        { path: '', component: ProductsList },
        { path: 'login', component: LoginComponent },
        { path: 'register', component: RegisterComponent },
        { path: 'cart', 
          component: CartComponent,
          canActivate: [authGuard] },
        { path: 'products/:id', component: ProductDetailsComponent },
        {
          path: 'farmer',
          component: FarmerDashboardComponent,
          canActivate: [authGuard, roleGuard],
          data: { role: 'Farmer' }
        },
        {
          path: 'orders',
          component: OrdersComponent,
          canActivate: [authGuard]
        }



];
