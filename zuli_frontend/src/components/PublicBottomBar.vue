<template>
  <footer class="public-bottom-bar">
    <div class="bottom-content">
        <div>© 2026 Zuli Airplanes. Todos los derechos reservados.</div>
        <div>Tel. (506) 1234-5678</div>
    </div>
  </footer>
</template>

<script>
export default {}
</script>

<style scoped>
.public-bottom-bar {
  width: 100%;
  background-color: #711717;
  color: white;
  padding: 18px 24px;
  box-sizing: border-box;
}

.bottom-content {
  display: flex;
  justify-content: space-around;
  align-items: center;
  gap: 20px;
  font-weight: 600;
}

@media (max-width: 768px) {
  .bottom-content {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>