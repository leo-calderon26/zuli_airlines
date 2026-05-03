<template>
<div class="sm:hidden">
    <label for="tabs-icons" class="sr-only">Select your country</label>
    <select id="tabs-icons" class="block w-full px-3 py-2.5 bg-neutral-secondary-medium border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand px-3 py-2.5 shadow-xs placeholder:text-body">
        <option>List Aircrafts</option>
        <option>Create Aircraft</option>
    </select>
</div>
<ul class="hidden text-sm font-medium text-center text-body sm:flex -space-x-px">
    <li class="w-full focus-within:z-10">
        <router-link to="/admin/aircrafts/">
            <a href="#" class="inline-flex items-center justify-center w-full text-body bg-neutral-primary-soft border border-default rounded-s-base hover:bg-purple hover:text-heading focus:ring-4 focus:ring-neutral-secondary-strong font-medium leading-5 text-sm px-4 py-2.5 focus:outline-none" aria-current="page">
                <p class="text-primary">List Aircrafts</p>
            </a>
        </router-link>
    </li>
    <li class="w-full focus-within:z-10">
        <router-link to="/admin/create-aircraft">
            <a href="#" class="inline-flex items-center justify-center  w-full text-body bg-neutral-primary-soft border border-default hover:bg-purple hover:text-heading focus:ring-4 focus:ring-neutral-secondary-strong font-medium leading-5 text-sm px-4 py-2.5 focus:outline-none">
                <p class="text-primary">Create Aircraft</p>
            </a>
        </router-link>
    </li>
</ul>

</template>

<script>
    export default {
        
    }
</script>

<style>
.top-nav-bar {
    background-color: #711717;
    width: 100%;
}

.content {
  width: 100%;
  display: flex;
  justify-content: flex-start;
  align-items: center;
}

.top-content {
    width: 100%;
    align-items: center;
    gap: 20px;
    font-weight: 600;
    color: white;
    font-size: 26px;
}

.logo {
  width: auto;
  height: clamp(48px, 8vw, 100px);
  max-width: 100%;
}

.admin-link {
  margin-left: auto;
  margin-right: 20px;
  color: white;
  text-decoration: none;
  font-weight: 600;
  display: flex;
  flex-direction: column;
  align-items: center;
}

nav ul {
  display: flex; 
  align-items: center;
  list-style: none;
  margin: 0;
  padding: 0;
}

nav ul li {
  padding: 0 18px;
}

nav ul li + li {
  border-left: 1px solid rgba(255, 255, 255, 0.5);
}

.nav-links {
  color: white;
  text-decoration: none;
  font-weight: 600;
}

.nav-links a {
  color: white;
  text-decoration: none;
  font-weight: 600;
  padding: 10px 14px;
  display: inline-block;
}

.nav-links a.router-link-exact-active {
  background-color: rgba(0, 0, 0, 0.25);
  border-radius: 6px;
}

</style>